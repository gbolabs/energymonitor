import {Component} from '@angular/core';
import {Measure} from 'src/model/Measure';
import {MeasuresService} from 'src/services/measures.service';
import {DailyProduction} from "src/model/dailyProduction";
import {Production} from "src/services/production";

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
  constructor(private measureService: MeasuresService) {
    this.productionYesterday = new DailyProduction();
    this.productionToday = new DailyProduction();
    this.latestProduction = new Production();
  }

  ngOnInit(): void {
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
}
