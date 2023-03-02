import { Injectable } from '@angular/core';
import { MeasuresService } from "./measures.service";
import { forkJoin, interval, map, mergeAll, Observable, range, switchMap } from "rxjs";
import { DailyMeasure, Measure } from 'src/model/Measure';
import { ReadVarExpr, ResourceLoader } from '@angular/compiler';
import { __values } from 'tslib';

@Injectable({
  providedIn: 'root'
})
export class GraphsService {

  // Inject MeasuresService into the constructor
  private measuresService: MeasuresService;

  constructor(measuresService: MeasuresService) {
    this.measuresService = measuresService;
  }

  getSeries() {
    return [
      this.getWeekEnergyConsumptionSeries(),
      this.getWeekSolarSeries()
    ];
  }

  private getWeekSolarData(): number[] {
    let array: number[] = [];
    for (let i = 6; i >= 0; i--) {
      let v = this.measuresService.getSolarProduction(i)
        .subscribe((value) => {
          let v = value?.productionKwh;
          array.push(value?.productionKwh ?? 0);
        }
        );
    }
    return array;
  }

  public getWeekEnergyConsumptionSeries(): Observable<DailyMeasure> {
    let a = range(0, 10).pipe(map((value) => {
      let day = new Date(Date.now() - (value * 24 * 60 * 60 * 1000));
      return this.measuresService.getMeasureOfDay(day)
        .pipe(map((value) => {
          return {
            date: day,
            in: value?.inHigh ?? 0 + value?.inLow ?? 0,
            out: value?.out ?? 0,
          } as DailyMeasure;
        }));
      // this.measuresService.getSolarProduction(value*-1).subscribe((value) => {
      //   dayMeasure.solar = value?.productionKwh ?? 0;
      // });
    }));
    let b = a.pipe(mergeAll()).pipe(map((value) => {
      // number of days in the past
      let days = Math.floor((Date.now() - value.date.getTime()) / (1000 * 60 * 60 * 24));
      return this.measuresService.getSolarProduction(days*-1)
        .pipe(map((s) => {
          return { date: value.date, in: value.in, out: value.out, solar: s?.productionKwh ?? 0 } as DailyMeasure;
        }));
    }));
    return b.pipe(mergeAll());
  }

  private getWeekSolarSeries() {
      return {
        name: 'Solar',
        type: 'line',
        stack: 'Total',
        areaStyle: {},
        emphasis: {
          focus: 'series'
        },
        data: this.getWeekSolarData()
      }
    }

  private getWeekEnergyConsumptionData() {
      let array: number[] = [];
      // add 5 fakes 0 values to the array
      for(let i = 0; i< 5; i++) {
      array.push(0);
    }

    this.measuresService.getMeasureToday().subscribe((value) => {
      array.push(value.inHigh + value.inLow);
    });
    this.measuresService.getMeasureYesterday().subscribe((value) => {
      array.push(value.inHigh + value.inLow);
    });
    return array;
  }
}
