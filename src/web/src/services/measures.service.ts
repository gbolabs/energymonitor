import { Injectable } from '@angular/core';
import { Measure } from '../model/Measure';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DatePipe } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class MeasuresService {

  // private hostname = 'http://localhost:49154/';  // URL to web api
  private hostname = 'https://app-pr114-energyapi-01.azurewebsites.net/';  // URL to web api
  private api = 'api/v1/measures/';

  constructor(
    private _datePipe: DatePipe,
    private http: HttpClient) {

  }
  getMeasureMin(minutes: number): Observable<Measure> {
    return this.http.get<Measure>(this.hostname + this.api + "last?minutes=" + minutes);
  }
  getMeasureToday(): Observable<Measure> {
    return this.http.get<Measure>(this.hostname + this.api + "today");
  }
  getMeasureYesterday(): Observable<Measure> {
    var yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    var yesterdayString = this._datePipe.transform(yesterday, "yyyy-MM-dd");
    return this.http.get<Measure>(this.hostname + this.api + "/date/" + yesterdayString);
  }
  getLastWeek(): Observable<Measure> {
    return this.http.get<Measure>(this.hostname + this.api + "/days/last/7");
  }
}
export const MEASURE: Measure = {
  duration: "00:10:00",
  inHigh: 10,
  inLow: 0,
  out: 0.1
}
