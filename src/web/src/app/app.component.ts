import { Component } from '@angular/core';
import { Measure } from 'src/model/Measure';
import { MeasuresService } from 'src/services/measures.service';
import { DailyProduction } from "src/model/dailyProduction";
import { Production } from "../model/production";
import * as echarts from 'echarts';
import { GraphsService } from "../services/graphs.service";
import { map, Observable, observable, toArray, of } from 'rxjs';
import { TypeModifier } from '@angular/compiler';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  measure30: Measure | undefined;
  measure2h: Measure | undefined;
  measureToday: Measure | undefined;
  measureYesterday: Measure | undefined;
  lastWeek: Measure | undefined;
  productionToday: DailyProduction;
  title = 'Energy Report';
  productionYesterday: DailyProduction;
  latestProduction: Production;

  /**
   *
   */
  constructor(private measureService: MeasuresService, private graphServices: GraphsService) {
    this.productionYesterday = new DailyProduction();
    this.productionToday = new DailyProduction();
    this.latestProduction = new Production();
  }

  ngOnInit(): void {

    this.initGraphs();

    this.refreshMeasures();
  }

  refreshMeasures(): void {
    this.measureService.getMeasureMin(30)
      .subscribe(measure => this.measure30 = measure);
    this.measureService.getMeasureMin(240)
      .subscribe(measure => this.measure2h = measure);
    this.measureService.getMeasureToday()
      .subscribe(measure => this.measureToday = measure);
    this.measureService.getTodayProduction()
      .subscribe(production => this.productionToday = production);
    this.measureService.getSolarProduction(1)
      .subscribe(production => this.productionYesterday = production);
    this.measureService.getMeasureYesterday()
      .subscribe(measure => this.measureYesterday = measure);
    this.measureService.getLastWeek()
      .subscribe(measure => this.lastWeek = measure);
    this.measureService.getLatestProduction().subscribe(production => this.latestProduction = production);
  }

  private initGraphs() {
    var myChart = echarts.init(document.getElementById('graph') as HTMLDivElement);

    this.graphServices.getWeekEnergyConsumptionSeries().pipe(map(series => series.in))
      .subscribe(inArray => {
        // inSeries.data=inArray;
      });

    let inSeries = {
      name: 'Grid',
      type: 'line',
      stack: 'total',
      smooth: false,
      areaStyle: {
        color: 'blue'
      },
      yAxisIndex: 0,
      emphasis: {
        focus: 'series'
      },
      label: {
        show: true,
        position: 'top',
        color: 'blue'
      },
      data: [0]
    };
    let outSeries = {
      name: 'Injection',
      type: 'line',
      smooth: false,
      // stack: 'total',
      // yAxisIndex: 1,
      label: {
        show: true,
        position: 'bottom',
        color: 'red'
      },
      areaStyle: {
        color: 'red'
      },
      emphasis: {
        focus: 'series'
      },
      data: [0]
    };
    let solarSeries = {
      name: 'Solar',
      type: 'line',
      stack: 'total',
      smooth: false,
      areaStyle: {
        color: 'orange'
      },
      yAxisIndex: 0,
      emphasis: {
        focus: 'series'
      },
      label: {
        show: true,
        position: 'top',
        color: 'orange'
      },
      data: [0]
    };

    var options = {
      title: {
        text: 'Stacked Area Chart'
      },
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'cross',
          label: {
            backgroundColor: '#6a7985'
          }
        }
      },
      xAxis: {
        type: 'category',
        boundaryGap: false,
        axisLabel: {
          formatter: 'Day-{value}'
        },
        axisLine: {
          show: true
        },
        data: [6, 5, 4, 3, 2, 1, 0]
      },
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
            formatter: '{value} kWh'
          }
        }
      ],
      series: [
        inSeries,
        solarSeries,
        outSeries
      ]
    }

    this.graphServices.getWeekEnergyConsumptionSeries()
      .pipe(toArray())
      .pipe(map(series => series.sort((a, b) => a.date.getTime() - b.date.getTime())))
      // .pipe(map(series => series.map(s =>s.in)))
      .subscribe(x => {
        console.log(x);
        inSeries.data = x.map(s => {
          console.log(s.in);
          return s.in;
        })
        outSeries.data = x.map(s => s.out * -1)
        solarSeries.data = x.map(s => s.solar)

        myChart.setOption(options);
      });

    console.log('option');
    console.log(myChart.getOption());
  }
}

