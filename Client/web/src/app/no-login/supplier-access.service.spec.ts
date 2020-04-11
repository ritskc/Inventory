import { TestBed } from '@angular/core/testing';

import { SupplierAccessService } from './supplier-access.service';

describe('SupplierAccessService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SupplierAccessService = TestBed.get(SupplierAccessService);
    expect(service).toBeTruthy();
  });
});
