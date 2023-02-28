import {Injectable} from '@angular/core';
import {MeasuresService} from "./measures.service";
import {Observable} from "rxjs";

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
      console.log(i);
      let v = this.measuresService.getSolarProduction(i)
        .subscribe((value) => {
            let v = value?.productionKwh;
            console.log(v);
            array.push(value?.productionKwh ?? 0);
          }
        );
    }
    return array;
  }

  public getWeekEnergyConsumptionSeries(): number[] {
    return this.getWeekEnergyConsumptionData();
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
    for (let i = 0; i < 5; i++) {
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
