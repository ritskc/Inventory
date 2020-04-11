import { Injectable } from '@angular/core';
import { ApiService } from '../common/services/api.service';
import { ConfigService } from '../config/config.service';
import { Company } from '../models/company.model';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {

  private _currentlyLoggedInCompanyId: number = 0;

  constructor(private apiService: ApiService, private config: ConfigService) { 
    this._currentlyLoggedInCompanyId = 1;
  }

  getGridPriviledges(username: string): any {
    return this.apiService.get<any>(`${ this.config.Settings.apiServerHost }/${ this.config.Settings.gridDefinitionUri }/${ username }`);
  }

  getAllCompanies() {
    return this.apiService.get<Company[]>(this.config.Settings.apiServerHost + this.config.Settings.companyUri);
  }

  getCompany(id: number) {
    return this.apiService.get<Company>(this.config.Settings.apiServerHost + this.config.Settings.companyUri + '/' + id);
  }

  //TODO: Replace the logic once the logged in companyId is determined
  getCurrentlyLoggedInCompanyId(): number {
    return this._currentlyLoggedInCompanyId;
  }

  setHarisons() {
    this._currentlyLoggedInCompanyId = 1;
  }

  setCastAndForge() {
    this._currentlyLoggedInCompanyId = 2;
  }

  saveCompany(company: Company) {
    if (company.id == 0)
      return this.apiService.post(company, this.config.Settings.apiServerHost + this.config.Settings.companyUri);
    else
      return this.apiService.put(company, this.config.Settings.apiServerHost + this.config.Settings.companyUri + `/${ company.id }`);
  }

  deleteCompany(id: number) {
    return this.apiService.delete(id, this.config.Settings.apiServerHost + this.config.Settings.companyUri);
  }
}