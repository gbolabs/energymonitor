import { Component } from '@angular/core';
import { Measure } from 'src/model/Measure';
import { MeasuresService } from 'src/services/measures.service';

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
  title = 'Energy Report';
  /**
   *
   */
  constructor(private measureService: MeasuresService) {

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
  }
}
