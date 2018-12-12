import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  displayName: String;

  constructor(
    private alertify: AlertifyService,
    private authService: AuthService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.displayName = this.authService.decodedToken.unique_name;
      this.alertify.success('Logged in successfully');
    }, error => {
      this.alertify.error(error);
    });
  }

  loggedIn = (): Boolean => {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    this.alertify.message('Logged out.');
  }

}
