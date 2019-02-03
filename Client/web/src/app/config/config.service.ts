import { Injectable } from "@angular/core";
import { Observable, of } from 'rxjs';
import { AppConfigurations } from './app.config';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  // getAllConfigurations(): Observable<AppConfigurations> {
  //   let configurations = new AppConfigurations();
  //   return of(configurations);
  // }

  get Settings(): AppConfigurations {
    return new AppConfigurations();
  }

}