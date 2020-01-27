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

  delete(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }`);
  }

  updateOpeningQuantity(part: Part, companyId: number, partId: number, quantity: number) {
    return this.apiService.put<Part>(part, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/OpeningQty/${ partId }/${ quantity }`);
  }

  updateOpeningQuantityByPartCode(part: Part, companyId: number, partId: number, quantity: number) {
    return this.apiService.put<Part>(part, `${ this.configService.Settings.apiServerHost }${ this.configService.Settings.partsUri }/${ companyId }/${ partId }/${ quantity }`);
  }

  adjustPart(partId: number, direction: string, notes: string, companyId: number, quantity: number) {
    return this.apiService.post<Part>(null, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/${ partId }/${ direction }/${ quantity }/${ notes }`);
  }

  getPartsStatus(companyId: number, partId: number, type: string): Observable<any> {
    return this.apiService.get<any>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/${ type }/${ partId }`);
  }
}