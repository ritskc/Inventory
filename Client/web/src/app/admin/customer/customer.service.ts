import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Observable } from 'rxjs';
import { Customer } from '../../models/customer.model';
import { PurchaseOrder } from '../../models/purchase-order';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  getAllCustomers(companyId: number): Observable<Customer[]> {
    return this.apiService.get<Customer[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.customerUri }/${ companyId }`);
  }

  getCustomer(companyId: number, customerId: number): Observable<Customer> {
    return this.apiService.get<Customer>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.customerUri }/${ companyId }/${ customerId }`);
  }

  getAllPurchaseOrders(companyId: number): Observable<PurchaseOrder[]> {
    return this.apiService.get<PurchaseOrder[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.ordersUri }/${ companyId }`)
  }

  getPurchaseOrder(companyId: number, orderId: number): Observable<PurchaseOrder> {
    return this.apiService.get<PurchaseOrder>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.ordersUri }/${ companyId }/${ orderId }`)
  }

  saveCustomer(customer: Customer) {
    if (customer.id == 0) 
      return this.apiService.post(customer, this.configService.Settings.apiServerHost + this.configService.Settings.customerUri);
    else
      return this.apiService.put(customer, this.configService.Settings.apiServerHost + this.configService.Settings.customerUri + `/${ customer.id }`);
  }

  savePurchaseOrder(purchaseOrder: PurchaseOrder) {
    if (purchaseOrder.id == 0)
      return this.apiService.post(purchaseOrder, this.configService.Settings.apiServerHost + this.configService.Settings.ordersUri);
    else
      return this.apiService.put(purchaseOrder, this.configService.Settings.apiServerHost + this.configService.Settings.ordersUri + `/${ purchaseOrder.id }`);
  }

  deletePurchaseOrder(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.ordersUri }`);
  }

  delete(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.customerUri }`)
  }

  getSaleReport(companyId: number, from: string, to: string): Observable<any[]> {
    return this.apiService.get(`${this.configService.Settings.apiServerHost}/reports/sales/${companyId}/${ from }/${ to }`);
  }
}
