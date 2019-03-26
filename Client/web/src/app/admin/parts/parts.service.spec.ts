import { TestBed } from '@angular/core/testing';

import { PartsService } from './parts.service';

describe('PartsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PartsService = TestBed.get(PartsService);
    expect(service).toBeTruthy();
  });
});
