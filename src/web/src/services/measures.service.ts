import {Injectable} from '@angular/core';
import {Measure} from '../model/Measure';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {DatePipe, Time} from '@angular/common';
import {DailyProduction} from "src/model/dailyProduction";
import {Production} from "src/model/production";

@Injectable({
  providedIn: 'root'
})
export class MeasuresService {

  private hostname = 'http://localhost:5000/';  // URL to web api
  // private hostname = 'https://app-pr114-energyapi-01.azurewebsites.net/';  // URL to web api
  private api = 'api/v1/measures/';
  private apiProduction = 'api/v3/production/';

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

  getMeasureOfDay(day: Date): Observable<Measure> {
    var dayString = this._datePipe.transform(day, "yyyy-MM-dd");
    return this.http.get<Measure>(this.hostname + this.api + "/date/" + dayString);
  }

  getLastWeek(): Observable<Measure> {
    return this.http.get<Measure>(this.hostname + this.api + "/days/last/7");
  }

  getTodayProduction(): Observable<DailyProduction> {
    return this.getSolarProduction(0);
  }

  getYesterdayProduction(): Observable<DailyProduction> {
    const v = this.getSolarProduction(1);
    if (v == null)
      return new Observable<DailyProduction>();
    return v;
  }

  getLatestProduction(): Observable<Production> {
    return this.http.get<Production>(this.hostname + this.apiProduction + "solar/last");
  }

  getSolarProduction(offset: number): Observable<DailyProduction> {
    return this.http.get<DailyProduction>(this.hostname + this.apiProduction + "solar/day/" + offset);
  }

  getEnergyReportForRange(date: Date, fromTime: Date, toTime: Date): Observable<Measure[]> {
    var dateString = this._datePipe.transform(date, "yyyy-MM-dd");
    var fromString = this._datePipe.transform(fromTime, "HH:mm");
    var toString = this._datePipe.transform(toTime, "HH:mm");
    return this.http.get<Measure[]>(this.hostname + this.api + "range/" + dateString + "/" + fromString + "/" + toString + "/grouped/00:10:00");
  }
}
