import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Observable } from 'rxjs';
import { Customer } from '../../models/customer.model';

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

  saveCustomer(customer: Customer) {
    if (customer.id == 0) 
      return this.apiService.post(customer, this.configService.Settings.apiServerHost + this.configService.Settings.supplierUri);
    else
      return this.apiService.put(customer, this.configService.Settings.apiServerHost + this.configService.Settings.supplierUri + `/${ customer.id }`);
  }
}
