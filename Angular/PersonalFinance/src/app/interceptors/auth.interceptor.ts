import { HttpInterceptorFn } from '@angular/common/http';
import { catchError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const token = localStorage.getItem('access_token');

    if (token) {
        const clonedReq = req.clone({
            headers: req.headers.set('Authorization', `Bearer ${token}`)
        });
        return next(clonedReq).pipe(
            // token expirado
            catchError(err => {
                if (err.status === 401) {
                    localStorage.removeItem('access_token');
                    window.location.href = '/login';
                }
                throw err;
            })
        );
    }

    return next(req);
};