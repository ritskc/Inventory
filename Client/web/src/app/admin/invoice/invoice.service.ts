import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Observable } from 'rxjs';
import { Invoice } from '../../models/invoice.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  getAllInvoices(companyId: number): Observable<Invoice[]> {
    return this.apiService.get<Invoice[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/${ companyId }`);
  }

  getAllSupplierInvoices(companyId: number, supplierId: number): Observable<Invoice[]> {
    return this.apiService.get<Invoice[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/${ companyId }/${ supplierId }`);
  }

  getSupplierInoice(companyId: number, invoiceId: number) : Observable<Invoice> {
    return this.apiService.get<Invoice>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/${ companyId }/${ invoiceId }`);
  }

  receivedInvoice(supplierId: number, invoiceId: number) {
    return this.apiService.post<number>(invoiceId, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.invoiceUri }/receive/${ invoiceId }`);
  }
}