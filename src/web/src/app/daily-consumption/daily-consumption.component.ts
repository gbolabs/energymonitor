import { Component } from '@angular/core';
import { MeasuresService } from "../../services/measures.service";
import { GraphsService } from "../../services/graphs.service";

declare var echarts: any;

@Component({
  selector: 'app-daily-consumption',
  templateUrl: './daily-consumption.component.html',
  styleUrls: ['./daily-consumption.component.css']
})
export class DailyConsumptionComponent {

  private _gaugeOptions: any;
  private _gridLine: any = {
    name: 'Grid',
    type: 'line',
    stack: 'Total',
    emphasis: {
      focus: 'series'
    },
    // 24 random values between 0 and 20
    data:[0],
    areaStyle: {}
  };
  private _solarLine: any = {
    name: 'Solar',
    type: 'line',
    stack: 'Total',
    emphasis: {
      focus: 'series'
    },
    data:[0],
    areaStyle: {}
  };
  private _injectionLine: any = {
    name: 'Injection',
    type: 'line',
    emphasis: {
      focus: 'series'
    },
    data:[0],
    areaStyle: {
      color: '#FF0000'
    }
  };

  constructor(private measuresService: MeasuresService, private production: GraphsService) {
  }

  ngOnInit(): void {
    this.refreshSeries();
    this.initGraphs();
  }
  private refreshSeries() {

  }


  private buildOptions() {
    function getDayHours() {
      let hours = [];
      for (let i = 0; i < 24; i++) {
        hours.push(i + ':00');
      }
      return hours;
    }

    return {
      xAxis: [{
        type: 'category',
        data: getDayHours(),
        boundaryGap: false
      }],
      yAxis: [{
        type: 'value',
        axisLabel: {
          formatter: '{value} kWh'
        }
      }],
      series: [
        this._gridLine,
        this._solarLine,
        this._injectionLine
      ]
    }
  }

  private initGraphs() {
    var dom = document.getElementById('consumptionDailyGraph')
    let dailyConsumptionGraph = echarts.init(dom);
    this._gaugeOptions = this.buildOptions();
    dailyConsumptionGraph.showLoading();
    dailyConsumptionGraph.setOption(this._gaugeOptions);
    dailyConsumptionGraph.hideLoading();
  }
}
