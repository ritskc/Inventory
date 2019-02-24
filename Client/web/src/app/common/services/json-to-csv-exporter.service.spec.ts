import { TestBed } from '@angular/core/testing';

import { JsonToCsvExporterService } from './json-to-csv-exporter.service';

describe('JsonToCsvExporterService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: JsonToCsvExporterService = TestBed.get(JsonToCsvExporterService);
    expect(service).toBeTruthy();
  });
});
