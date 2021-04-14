import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../common/services/api.service';
import { CompanyService } from '../../company/company.service';
import { ConfigService } from '../../config/config.service';

@Injectable({
  providedIn: 'root'
})
export class ContainerService {

  currentlyLoggedInCompany: number = 0;

  constructor(private configService: ConfigService, private apiService: ApiService, private companyService: CompanyService) { }

  getAllContainersInACompany(): Observable<any> {
    var companyId = this.companyService.getCurrentlyLoggedInCompanyId();
    return this.apiService.get<any>(`${ this.configService.Settings.apiServerHost }/containers/${ companyId }`);
  }
}