import { Injectable } from '@angular/core';
import { Measure } from '../model/Measure';

@Injectable({
  providedIn: 'root'
})
export class MeasuresService {

  constructor() { }

  getMeasure(): Measure {
    return MEASURE
  }
}
export const MEASURE: Measure = {
  duration: "00:10:00",
  inHigh: 10,
  inLow: 0,
  out: 0.1
}