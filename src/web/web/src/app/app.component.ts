import { Component } from '@angular/core';
import { Measure } from 'src/model/Measure';
import { MeasuresService } from 'src/services/measures.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  measure: Measure = { duration: "", inHigh: 0, inLow: 0, out: 0 };
  title = 'web';
  /**
   *
   */
  constructor(private measureService: MeasuresService) {

  }

  getMeasure(): void {
    this.measure = this.measureService.getMeasure();
  }
}
