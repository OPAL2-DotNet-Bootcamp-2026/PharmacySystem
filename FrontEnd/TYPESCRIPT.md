# TypeScript on the login and pharmacist-order pages

The browser still loads `JS/login.js` and `JS/pharmacist-order.js`. Those files
are now generated from the TypeScript source in `TS/`.

## Build once

From the `FrontEnd` folder:

```powershell
npm install
npm run build
```

## Rebuild automatically while editing

```powershell
npm run watch
```

Edit these source files instead of editing the generated JavaScript:

- `TS/login.ts`
- `TS/pharmacist-order.ts`
- `TS/globals.d.ts` for the shared `Auth` and `Api` declarations

The strict settings in `tsconfig.json` check API response shapes, request
bodies, DOM element types, user roles, order status values, and caught errors.
