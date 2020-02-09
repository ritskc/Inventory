import { Injectable } from '@angular/core';
import { ApiService } from '../common/services/api.service';
import { ConfigService } from '../config/config.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SupplierAccessService {

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  getDirectSupplierPurchaseOrder(id: string): Observable<any> {
    return this.apiService.get(`${this.configService.Settings.apiServerHost}/${this.configService.Settings.directSupplierPo}/${id}`);
  }

  submitSupplierPurchaseOrder(supplierOrder: any, accessId: string) {
    return this.apiService.put<any>(supplierOrder, `${this.configService.Settings.apiServerHost}/${this.configService.Settings.directSupplierPo}/acknowledge/${accessId}`);
  }
}
