import { TestBed } from '@angular/core/testing';

import { InventoryService } from './inventory.service';
import { HttpClientModule } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('InventoryService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      HttpClientModule,
      HttpClientTestingModule
    ]
  }));

  it('should be created', () => {
    const service: InventoryService = TestBed.get(InventoryService);
    expect(service).toBeTruthy();
  });
});
