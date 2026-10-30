# Mini ERP Web

Angular frontend for Mini ERP, generated with Angular CLI 21.

The app has a login page, an inventory table with SKU/name search, product create/edit/detail pages, and global/per-product activity history. Product API requests use `/api`; the development server proxies them to `http://localhost:5285`.

## Requirements

- Node.js 20.19.x, 22.12+, or 24+
- npm

## Development server

Install dependencies and start the development server:

```powershell
npm install
npm start
```

Open `http://localhost:4200/`. The application reloads when source files change.

## Code scaffolding

Generate a component with Angular CLI:

```powershell
npm exec ng generate component component-name
```

## Build and tests

```powershell
npm run build
npm test
```

The production build is written to `dist/`.
