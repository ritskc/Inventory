import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Observable } from 'rxjs';
import { Part } from '../../models/part.model';

@Injectable({
  providedIn: 'root'
})
export class PartsService {

  constructor(private apiService: ApiService, private configService: ConfigService,) { 
    
  }

  getAllParts(companyId: number): Observable<Part[]> {
    return this.apiService.get<Part[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }`);
  }

  getPart(companyId: number, partId: number): Observable<Part> {
    return this.apiService.get<Part>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/${ partId }`);
  }

  save(part: Part) {
    if (part.id > 0) 
      return this.apiService.put<Part>(part, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ part.id }`);
    else
      return this.apiService.post<Part>(part, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }`);
  }
}