import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SolarGaugeComponent } from './solar-gauge.component';

describe('SolarGaugeComponent', () => {
  let component: SolarGaugeComponent;
  let fixture: ComponentFixture<SolarGaugeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SolarGaugeComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SolarGaugeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
