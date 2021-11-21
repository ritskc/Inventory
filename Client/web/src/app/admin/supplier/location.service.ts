import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Box } from '../../models/box.model';
import { Location } from '../../models/location.model';

@Injectable({
  providedIn: 'root'
})
export class LocationService {

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  getAllLocations(companyId: number): Observable<Location[]> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.locationUri }/${ companyId }`)
  }

  getAllBoxes(companyId: number): Observable<Box[]> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.boxUri }/${ companyId }`)
  }

  getLocation(companyId: number, locationId: number) : Observable<Location> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.locationUri }/${ companyId }/${ locationId }`);
  }

  getBox(companyId: number, boxId: number) : Observable<Box> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.boxUri }/${ companyId }/${ boxId }`);
  }

  save(location: Location) {
    if (location.id == 0) 
      return this.apiService.post(location, this.configService.Settings.apiServerHost + this.configService.Settings.locationUri);
    else
      return this.apiService.put(location, this.configService.Settings.apiServerHost + this.configService.Settings.locationUri + `/${ location.id }`);
  }

  saveBox(box: Box) {
    if (box.id == 0) 
      return this.apiService.post(box, this.configService.Settings.apiServerHost + this.configService.Settings.boxUri);
    else
      return this.apiService.put(box, this.configService.Settings.apiServerHost + this.configService.Settings.boxUri + `/${ box.id }`);
  }

  delete(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.locationUri }`)
  }

  deleteBox(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.boxUri }`)
  }
}
