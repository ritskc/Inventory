import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Observable } from 'rxjs';
import { Supplier } from '../../models/supplier.model';
import { PurchaseOrder } from '../../models/purchase-order';
import * as DateHelper from '../../common/helpers/dateHelper';
import { Invoice } from '../../models/invoice.model';

@Injectable({
  providedIn: 'root'
})
export class SupplierService {

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  getAllSuppliers(companyId: number): Observable<Supplier[]> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.supplierUri }/${ companyId }`)
  }

  getSupplier(companyId: number, supplierId: number): Observable<Supplier> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.supplierUri }/${ companyId }/${ supplierId }`);
  }

  getPurchaseOrders(companyId: number): Observable<PurchaseOrder[]> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.posUri }/${ companyId }`);
  }

  getPurchaseOrder(companyId: number, purchaseOrderId: number): Observable<PurchaseOrder> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.posUri }/${ companyId }/${ purchaseOrderId }`);
  }

  saveSupplier(supplier: Supplier) {
    if (supplier.id == 0) 
      return this.apiService.post(supplier, this.configService.Settings.apiServerHost + this.configService.Settings.supplierUri);
    else
      return this.apiService.put(supplier, this.configService.Settings.apiServerHost + this.configService.Settings.supplierUri + `/${ supplier.id }`);
  }

  delete(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.supplierUri }`)
  }

  savePurchaseOrder(purchaseOrder: PurchaseOrder) {
    if (purchaseOrder.id == 0)
      return this.apiService.post(purchaseOrder, this.configService.Settings.apiServerHost + this.configService.Settings.posUri);
    else 
      return this.apiService.put(purchaseOrder, this.configService.Settings.apiServerHost + this.configService.Settings.posUri + `/${ purchaseOrder.id }`);
  }

  deletePurchaseOrder(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }${ this.configService.Settings.posUri }`);
  }

  getNewPurchaseOrderNumber(companyId: number, date: string): Observable<any> {
    return this.apiService.get(`${this.configService.Settings.apiServerHost}/${this.configService.Settings.entityTracker}/po/${companyId}/${ date }`)
  }

  getPurchaseReport(companyId: number, from: string, to: string): Observable<any[]> {
    return this.apiService.get(`${this.configService.Settings.apiServerHost}/reports/purchase/${companyId}/${ from }/${ to }`);
  }

  getSupplierOpenInvoice(companyId: number, partId: number): Observable<any> {
    return this.apiService.get(`${this.configService.Settings.apiServerHost}/${ this.configService.Settings.invoiceUri }/openinvoice/${ companyId }/${ partId }`);
  }
}
