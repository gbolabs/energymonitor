import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DailyConsumptionComponent } from './daily-consumption.component';

describe('DailyConsumptionComponent', () => {
  let component: DailyConsumptionComponent;
  let fixture: ComponentFixture<DailyConsumptionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DailyConsumptionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DailyConsumptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
