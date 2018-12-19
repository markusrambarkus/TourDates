import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MemberEditResolver implements Resolve<User> {
    constructor(
        private alertify: AlertifyService,
        private authService: AuthService,
        private router: Router,
        private userService: UserService) {
    }

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(this.authService.decodedToken.nameid)
            .pipe(
                catchError(error => {
                    this.alertify.error('Problem retriving user data');
                    this.router.navigate(['/members']);
                    return of(null);
                })
            );
    }
}
