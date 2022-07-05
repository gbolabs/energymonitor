import { Component } from '@angular/core';
import { Measure } from 'src/model/Measure';
import { MeasuresService } from 'src/services/measures.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  measure: Measure | undefined;
  title = 'web';
  /**
   *
   */
  constructor(private measureService: MeasuresService) {

  }
  
  ngOnInit(): void {
    this.getMeasure();
  }

  getMeasure(): void {
    this.measureService.getMeasure()
      .subscribe(measure=>this.measure=measure);
  }
}
