import { Injectable } from '@angular/core';
import { Measure } from '../model/Measure';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MeasuresService {

  // private hostname = 'http://localhost:49154/';  // URL to web api
  private hostname = 'https://measures.wittybay-1b34bb22.westeurope.azurecontainerapps.io/';  // URL to web api
  private api = 'api/v1/measures/last?minutes=30';

  constructor(
    private http: HttpClient) {

  }

  getMeasure(): Observable<Measure> {
    //    return MEASURE
    console.log(this.hostname + this.api);
        
    const headerDict = {
      'Accept': 'application/json',
      'Access-Control-Allow-Headers': 'Content-Type',
      'Access-Control-Allow-Origin': '*',
    }
    
    const requestOptions = {                                                                                                                                                                                 
      headers: new HttpHeaders (headerDict), 
    };
    this.http.get(this.hostname);
    // return this.http.get<Measure>(this.hostname + this.api, requestOptions);
    return this.http.get<Measure>(this.hostname + this.api);
  }
}
export const MEASURE: Measure = {
  duration: "00:10:00",
  inHigh: 10,
  inLow: 0,
  out: 0.1
}