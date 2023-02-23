export class Production {
  get sampling(): Date {
    return this._sampling;
  }

  set sampling(value: Date) {
    this._sampling = value;
  }

  // Constructor
  constructor() {
    this._currentPowerW = 0;
    this._sampling = new Date();
  }

  get currentPowerW(): number {
    return this._currentPowerW;
  }

  set currentPowerW(value: number) {
    this._currentPowerW = value;
  }

  private _currentPowerW: number;
  private _sampling: Date;
}
