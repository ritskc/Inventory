import { Injectable } from "@angular/core";
import { Observable, of } from 'rxjs';
import { AppConfigurations, GridConstants } from './app.config';

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

  get GridConstants(): GridConstants {
    return new GridConstants();
  }
}