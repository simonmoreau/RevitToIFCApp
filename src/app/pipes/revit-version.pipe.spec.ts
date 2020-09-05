import { RevitVersionPipe } from './revit-version.pipe';

describe('RevitVersionPipe', () => {
  it('create an instance', () => {
    const pipe = new RevitVersionPipe();
    expect(pipe).toBeTruthy();
  });
});
