import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';


import { AppComponent } from './app.component';
import { CommonModule, DatePipe } from '@angular/common';
import { SolarGaugeComponent } from './solar-gauge/solar-gauge.component';

@NgModule({
  declarations: [
    AppComponent,
    SolarGaugeComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    CommonModule
  ],
  providers: [
    DatePipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
