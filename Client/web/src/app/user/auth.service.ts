import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, tap } from 'rxjs/operators';
import { ApiService } from '../common/services/api.service';
import { ConfigService } from '../config/config.service';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  isLoggedIn: boolean = false;
  redirectUrl: string = '';

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  login(loginname: string, password: string) {
    var user = new User();
    user.username = loginname;
    user.password = password;
    return this.apiService.post(user, this.configService.Settings.apiServerHost + this.configService.Settings.usersUri + '/authenticate');
  }

  logout(): void {
    this.isLoggedIn = false;
  }
}
