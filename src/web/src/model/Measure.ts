export interface Measure{
    duration: string,
    inHigh: number,
    inLow: number,
    out: number
}

export class DailyMeasure
{
  constructor() {
    this.date = new Date();
    this.in = 0;
    this.out = 0;
    this.solar =  0;
  }

  public date: Date;
  public in: number;
  public out: number;
  public solar: number;
}
