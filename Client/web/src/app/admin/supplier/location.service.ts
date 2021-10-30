import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Location } from '../../models/location.model';

@Injectable({
  providedIn: 'root'
})
export class LocationService {

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  getAllLocations(companyId: number): Observable<Location[]> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.locationUri }/${ companyId }`)
  }

  delete(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.locationUri }`)
  }
}
