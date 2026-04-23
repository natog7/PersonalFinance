import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class GeneralService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = 'https://localhost:55784/health';

    isOnline = signal<boolean>(false);

    checkOnline() {
        this.http.get(this.apiUrl).subscribe({
            next: (data) => this.isOnline.set(true),
            error: (err) => this.isOnline.set(false)
        });
    }
}
