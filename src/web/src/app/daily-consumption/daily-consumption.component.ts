import {Component} from '@angular/core';
import {MeasuresService} from "../../services/measures.service";
import {GraphsService} from "../../services/graphs.service";

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
    type: 'bar',
    stack: 'Total',
    label: {
      show: true,
      position: 'top',
      color: 'blue'
    },
    emphasis: {
      focus: 'series'
    },
    // 24 random values between 0 and 20
    data: [0],
    areaStyle: {
      color: 'blue'
    }
  };
  private _solarLine: any = {
    name: 'Solar',
    type: 'bar',
    stack: 'Total',
    label: {
      show: true,
      position: 'top',
      color: 'orange'
    },
    emphasis: {
      focus: 'series'
    },
    data: [0],
    areaStyle: {
      color: 'orange'
    }
  };
  private _injectionLine: any = {
    name: 'Injection',
    type: 'bar',
    emphasis: {
      focus: 'series'
    },
    data: [0],
    areaStyle: {
      color: '#FF0000'
    }
  };

  constructor(private measuresService: MeasuresService, private production: GraphsService) {
  }

  ngOnInit(): void {
    this.refreshSeries();
  }

  private refreshSeries() {
    var today = new Date();
    // 00:00
    var start = new Date(Date.now());
    start.setHours(0, 0, 0, 0);
    // 23:59
    var end = new Date(Date.now());
    end.setHours(23, 59, 0, 0);

    this.measuresService.getEnergyReportForRange(today, start, end)
      .subscribe((data) => {
        this._gridLine.data = data.map((x) => x.averagePowerIn);
        this._injectionLine.data = data.map((x) => x.averagePowerOut * -1);
        this._solarLine.data = data.map((x) => 0);
        this._gaugeOptions = this.buildOptions();
        console.log(this._gaugeOptions);
        echarts.init(document.getElementById('consumptionDailyGraph')).setOption(this._gaugeOptions);
      });
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
      yAxis: [
        {
          name: 'Energy',
          type: 'value',
          position: 'left',
          offset: 5,
          axisLine: {
            show: true
          },
          axisLabel: {
            formatter: '{value} Wh'
          }
        }
      ],
      xAxis: [{
        type: 'category',
        data: getDayHours(),
        boundaryGap: false
      }],
      series: [
        this._gridLine,
        this._solarLine,
        this._injectionLine
      ]
    }
  }
}
