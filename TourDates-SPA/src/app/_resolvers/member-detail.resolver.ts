import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberDetailResolver implements Resolve<User> {
    constructor(
        private alertify: AlertifyService,
        private router: Router,
        private userService: UserService) {
    }

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(route.params['id'])
            .pipe(
                catchError(error => {
                    this.alertify.error('Problem retrieving member data');
                    this.router.navigate(['/members']);
                    return of(null);
                })
            );
    }
}
