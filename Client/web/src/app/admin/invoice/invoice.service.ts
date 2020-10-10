import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Observable } from 'rxjs';
import { Invoice, UploadInvoice } from '../../models/invoice.model';
import { Shipment } from '../../models/shipment.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  getAllInvoices(companyId: number): Observable<Invoice[]> {
    return this.apiService.get<Invoice[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/${ companyId }`);
  }

  getInvoice(companyId: number, invoiceId: number): Observable<Invoice> {
    return this.apiService.get<Invoice>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/${ companyId }/${ invoiceId }`);
  }

  getAllSupplierInvoices(companyId: number, supplierId: number): Observable<Invoice[]> {
    return this.apiService.get<Invoice[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/${ companyId }/${ supplierId }`);
  }

  getSupplierInoice(companyId: number, invoiceId: number) : Observable<Invoice> {
    return this.apiService.get<Invoice>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/${ companyId }/${ invoiceId }`);
  }

  receivedInvoice(supplierId: number, invoiceId: number, warehouseId: number) {
    return this.apiService.post<number>(invoiceId, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/receive/${ invoiceId }/${ warehouseId }`);
  }

  unReceivedInvoice(supplierId: number, invoiceId: number) {
    return this.apiService.put<number>(invoiceId, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/unreceive/${ invoiceId }`);
  }

  receivedBox(box: string) {
    return this.apiService.post<string>(box, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/receive/box/${ box }`);
  }

  validateInvoice(invoce: UploadInvoice) {
    return this.apiService.post<UploadInvoice>(invoce, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/1`);
  }

  uploadInvoice(invoce: Invoice) {
    return this.apiService.post<Invoice>(invoce, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/2`);
  }

  createCustomerInvoice(shipment: Shipment) {
    return this.apiService.post<Shipment>(shipment, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.customerInvoiceUri }`);
  }

  updateInvoice(invoice: Invoice) {
    return this.apiService.put<Invoice>(invoice, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/${ invoice.id }`);
  }

  deleteInvoice(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }`);
  }

  getAllMonthlyInvoice(companyId: number): Observable<Shipment[]> {
    return this.apiService.get<Shipment[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.monthlyInvoiceUri }/${ companyId }`);
  }

  getAMonthlyInvoice(companyId: number, shipmentId: number): Observable<Shipment> {
    return this.apiService.get<Shipment>(this.configService.Settings.apiServerHost + this.configService.Settings.monthlyInvoiceUri + `/${ companyId }/` + shipmentId);
  }

  getPreviousMonthlyInvoiceNo(companyId: number, date: string): Observable<any> {
    return this.apiService.get(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.entityTracker }/MONTHLY_INVOICE/${ companyId }/${ date }`);
  }

  saveMonthlyInvoice(shipment: Shipment) {
    if (shipment.id == 0)
      return this.apiService.post(shipment, this.configService.Settings.apiServerHost + this.configService.Settings.monthlyInvoiceUri);
    else 
      return this.apiService.put(shipment, this.configService.Settings.apiServerHost + this.configService.Settings.monthlyInvoiceUri + `/${ shipment.id }`);
  }

  deleteMonthlyInvoice(invoiceId: number) {
    return this.apiService.delete(invoiceId, this.configService.Settings.apiServerHost + this.configService.Settings.monthlyInvoiceUri);
  }
}