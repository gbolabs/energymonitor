import { Component } from '@angular/core';
import {interval, map, Observable} from 'rxjs';
import { DailyProduction } from 'src/model/dailyProduction';
import { Production } from 'src/model/production';
import { MeasuresService } from 'src/services/measures.service';

declare var echarts: any;

@Component({
  selector: 'app-solar-gauge',
  templateUrl: './solar-gauge.component.html',
  styleUrls: ['./solar-gauge.component.css']
})
export class SolarGaugeComponent {
  get countDown(): number {
    return this._countDown;
  }
  get lastSampling(): Date {
    return this._lastSampling;
  }
  private _lastSampling!: Date;
  private _countDown!: number;
  private _lastRefresh!: Date;

  constructor(private measureService: MeasuresService) {

    // Create a timer that will update the countdown every 1 second
    this._countDownTimer = interval(1000);
    this._countDownTimer.subscribe(t => {
      this._countDown = 60 - (new Date().getSeconds());
      if (this._countDown === 60) {
        this.updateGauge();
      }
    });
  }
  private _power!: Observable<Production>;
  private _gaugeOptions: any;
  private _countDownTimer: Observable<any>;

  ngOnInit(): void {
    this._gaugeOptions = buildOptions();
    this.updateGauge();
  }

  // Get the value from API and update the gauge
  updateGauge() {
    this._power = this.measureService.getLatestProduction();
    this._power.subscribe(production => {
      this._lastSampling = production.sampling;
      this._gaugeOptions.series[0].data[0].value = production.currentPowerW;
      this.initGraphs();
      this._lastRefresh = new Date();
    });
  }

  initGraphs() {
    let gaugeChart = echarts.init(document.getElementById('solar-gauge'));
    gaugeChart.setOption(this._gaugeOptions);
  }
}
function buildOptions(): any{
  return {
  series: [{
    type: 'gauge',
    startAngle: 220,
    endAngle: -40,
    min: 0,
    max: 600,
    detail: { formatter: '{value}W' },
    data: [{ value: 50, name: 'Power' }]
  }]
};
}
