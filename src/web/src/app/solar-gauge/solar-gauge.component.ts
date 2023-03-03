import { Component } from '@angular/core';
import {interval, map, Observable} from 'rxjs';
import { DailyProduction } from 'src/model/dailyProduction';
import { Production } from 'src/model/production';
import { MeasuresService } from 'src/services/measures.service';
import {Time} from "@angular/common";

declare var echarts: any;

@Component({
  selector: 'app-solar-gauge',
  templateUrl: './solar-gauge.component.html',
  styleUrls: ['./solar-gauge.component.css']
})
export class SolarGaugeComponent {
  get countDown(): Date {
    return new Date(0, 0, 0, this._countDown.hours, this._countDown.minutes, this._countDown.seconds);
  }
  get lastSampling(): Date {
    return this._lastSampling;
  }
  private _lastSampling!: Date;
  private _countDown!: any;
  private _lastRefresh!: Date;

  constructor(private measureService: MeasuresService) {

    // Create a timer that will update the countdown every 1 second
    this._countDownTimer = interval(1000);


    this._countDownTimer.subscribe(_=>this.tick());
  }
  private _power!: Observable<Production>;
  private _gaugeOptions: any;
  private _countDownTimer: Observable<any>;

  ngOnInit(): void {
    this._gaugeOptions = buildOptions();
    this.updateGauge();
  }

  tick() {
    // delta between now and last refresh
    let delta = Math.floor((new Date().getTime() - this._lastRefresh.getTime()) / 1000);

    // if delta is greater than 240 seconds, refresh the data otherwise only update the countdow
    if (delta > 240) {
      this.updateGauge();
    }
    else {
      this._countDown = {
        hours: Math.floor((240 - delta) / 3600),
        minutes: Math.floor((240 - delta) % 3600 / 60),
        seconds: Math.floor((240 - delta) % 3600 % 60)
      };
    }
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
