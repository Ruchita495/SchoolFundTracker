import { test, expect } from '@playwright/test';

test('treasurer can log in and record a payment', async ({ page }) => {
  await page.goto('/');
  await page.getByLabel('Username').fill('treasurer');
  await page.getByLabel('Password').fill('Treasurer123!');
  await page.getByRole('button', { name: 'Sign in' }).click();

  await expect(page.getByRole('heading', { name: 'Fund Summary' })).toBeVisible();

  await page.getByLabel('Amount').fill('5');
  await page.getByRole('button', { name: 'Record payment' }).click();
  await expect(page.getByText('Payment recorded')).toBeVisible();
});

test('wrong password shows an error', async ({ page }) => {
  await page.goto('/');
  await page.getByLabel('Username').fill('admin');
  await page.getByLabel('Password').fill('nope');
  await page.getByRole('button', { name: 'Sign in' }).click();
  await expect(page.getByRole('alert')).toContainText('Invalid');
});
