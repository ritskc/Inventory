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

  getSupplier(companyId: number, supplierId: number): Observable<Supplier> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.supplierUri }/${ supplierId }`);
  }

  saveSupplier(supplier: Supplier) {
    if (supplier.id == 0) 
      return this.apiService.post(supplier, this.configService.Settings.apiServerHost + this.configService.Settings.supplierUri);
    else
    return this.apiService.put(supplier, this.configService.Settings.apiServerHost + this.configService.Settings.supplierUri + `/${ supplier.id }`);
  }
}
