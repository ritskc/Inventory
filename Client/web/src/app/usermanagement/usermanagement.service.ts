import { Injectable } from '@angular/core';
import { ConfigService } from '../config/config.service';
import { Observable } from 'rxjs';
import { ApiService } from '../common/services/api.service';

@Injectable({
  providedIn: 'root'
})
export class UsermanagementService {

  constructor(private configService: ConfigService, private apiService: ApiService) { }

  getAllMenuItems(): Observable<any> {
    return this.apiService.get<any>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.privilegesUri }`);
  }

  getAllPrivileges(): Observable<any> {
    return this.apiService.get<any>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.privilegesUri }/1`);
  }

  save(privilege) {
    if (privilege.id > 0) 
      return this.apiService.put<any>(privilege, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.privilegesUri }/${ privilege.id }`);
    else
      return this.apiService.post<any>(privilege, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.privilegesUri }`);
  }

  removePrivilege(id) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.privilegesUri }`);
  }

  getAllUsers(): Observable<any> {
    return this.apiService.get<any>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.usersUri }`);
  }

  saveUser(user) {
    if (user.id > 0) 
      return this.apiService.put<any>(user, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.usersUri }/${ user.id }`);
    else
      return this.apiService.post<any>(user, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.usersUri }`);
  }

  removeUser(id) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.usersUri }`);
  }
}
