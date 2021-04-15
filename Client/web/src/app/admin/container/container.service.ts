import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../common/services/api.service';
import { CompanyService } from '../../company/company.service';
import { ConfigService } from '../../config/config.service';
import { Container } from '../../models/container.model';

@Injectable({
  providedIn: 'root'
})
export class ContainerService {

  currentlyLoggedInCompany: number = 0;
  warehouseId: number = 0;

  constructor(private configService: ConfigService, private apiService: ApiService, private companyService: CompanyService) { }

  getAllContainersInACompany(): Observable<Container[]> {
    var companyId = this.companyService.getCurrentlyLoggedInCompanyId();
    return this.apiService.get<Container[]>(`${ this.configService.Settings.apiServerHost }/containers/${ companyId }`);
  }

  saveContainer(container: Container) {
    if (container.id == 0)
      return this.apiService.post(container, `${ this.configService.Settings.apiServerHost }/containers`);
    else 
      return this.apiService.put(container, `${ this.configService.Settings.apiServerHost }/containers`);
  }

  deleteContainer(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/containers/${ id }`);
  }

  receiveContainer(containerId: number, warehouseId: number) {
    return this.apiService.post('', `${ this.configService.Settings.apiServerHost }/containers/receive/${ containerId }/${ warehouseId }`);
  }

  unreceiveContainer(containerId: number) {
    return this.apiService.put('', `${ this.configService.Settings.apiServerHost }/containers/unreceive/${ containerId }`);
  }
}