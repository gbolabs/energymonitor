export class DailyProduction {
  get duration(): Date {
    return this._duration;
  }

  set duration(value: Date) {
    this._duration = value;
  }

  constructor() {
    this._productionKwh = 0;
    this._duration = new Date();
  }

  get productionKwh(): number {
    return this._productionKwh;
  }

  set productionKwh(value: number) {
    this._productionKwh = value;
  }

  private _productionKwh: number
  private _duration: Date
}
