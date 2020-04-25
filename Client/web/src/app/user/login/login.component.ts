import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { User } from '../../models/user.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  submitted: boolean = false;
  invalidCredentials: boolean = false;
  loginname: string = '';
  password: string = '';
  user: User
  
  constructor(private router: Router, private authService: AuthService) { 
    this.user = new User();
  }

  ngOnInit() {
    console.log(this.router.url);
    localStorage.clear();
  }

  login() {
    this.submitted = true;
    this.authService.login(this.loginname, this.password).subscribe(
      (user: any) => {
      if (user.token) {
        this.authService.isLoggedIn = true;
        localStorage.setItem('token', user.token);
        localStorage.setItem('username', this.loginname);
        this.authService.setPrivileges(user);
        let redirectUrl = localStorage.getItem('landingurl');
        //let redirectUrl = this.authService.redirectUrl ? this.authService.redirectUrl : (environment.isSupplier ? '/suppliers':  '/companies');
        this.router.navigate([redirectUrl]);
      } else {
        this.invalidCredentials = true;
      }
    })
  }

  handleKeyDown(event) {
    this.submitted = false;
    this.invalidCredentials = false;
  }
}