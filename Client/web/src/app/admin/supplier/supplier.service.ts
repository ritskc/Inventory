import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Observable } from 'rxjs';
import { Supplier } from '../../models/supplier.model';

@Injectable({
  providedIn: 'root'
})
export class SupplierService {

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  getAllSuppliers(companyId: number): Observable<Supplier[]> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.supplierUri }`)
  }
}
