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
}
