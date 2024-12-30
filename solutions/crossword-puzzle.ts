/*
 * Complete the 'crosswordPuzzle' function below.
 *
 * The function is expected to return a STRING_ARRAY.
 * The function accepts following parameters:
 *  1. STRING_ARRAY crossword
 *  2. STRING words
 */

function crosswordPuzzle(crossword: string[], words: string): string[] {
    const wordsArr = words.split(';');

    if (!wordsArr) {
      return crossword;
    }

    let crosswordSolved = [...crossword].map(row => row.split(''));

    const fitHorizontally = (word: string, col: number, row: number) => {
      if (word.length + col > crosswordSolved[row].length) {
        return false;
      }
      if (crosswordSolved[row][col] === '+') {
        return false;
      }
      if (col > 0 && crosswordSolved[row][col-1] !== '+') {
        return false;
      }
      if (crosswordSolved[row].length > word.length + col && crosswordSolved[row][col + word.length] !== '+') {
        return false;
      }

      let wordCol = 0;
      while (wordCol < word.length) {
        if (crosswordSolved[row][col+wordCol] !== word[wordCol] && crosswordSolved[row][col+wordCol] !== '-') {
          return false;
        }
        wordCol++;
      }
      if (crosswordSolved[row].length > col+wordCol && crosswordSolved[row][col+wordCol]!=='+') {
        return false;
      }
      return true;
    }

    const fitVertically = (word: string, col: number, row: number) => {
      if (word.length + row > crosswordSolved.length) {
        return false;
      }
      if (crosswordSolved[row][col] === '+') {
        return false;
      }
      if (row > 0 && crosswordSolved[row-1][col] !== '+') {
        return false;
      }

      let wordCol = 0;
      while (wordCol < word.length) {
        if (crosswordSolved[row+wordCol][col] !== word[wordCol] && crosswordSolved[row+wordCol][col] !== '-') {
          return false;
        }
        wordCol++;
      }

      if (crosswordSolved.length > row + wordCol && crosswordSolved[row + wordCol][col] !=='+') {
        return false;
      }
      return true;    
    }

    const fillHorizontally = (word: string, col: number, row: number) => {
      for (let wordCol = 0; wordCol < word.length; wordCol++) {        
        if (crosswordSolved[row][col+wordCol] === word[wordCol] || crosswordSolved[row][col+wordCol] === '-') {        
          crosswordSolved[row][col+wordCol] = word[wordCol];
        }
      }  
    }

    const fillVertically = (word: string, col: number, row: number) => {
      for (let wordCol = 0; wordCol < word.length; wordCol++) {
        if (crosswordSolved[row+wordCol][col] === word[wordCol] || crosswordSolved[row+wordCol][col] === '-') {        
          crosswordSolved[row+wordCol][col] = word[wordCol];
        }
      }  
    }

    const solve = (index: number): boolean => {
      if (index === wordsArr.length) {
        return true;
      }
      const word = wordsArr[index];

      for (let row = 0; row < crosswordSolved.length; row++) {
        for (let col = 0; col< crosswordSolved[row].length; col++) {
          if (fitHorizontally(word, col, row)) {
            fillHorizontally(word, col, row);
            if (solve(index+1)) {         
              return true;
            }
          } else if (fitVertically(word, col, row)) {
            fillVertically(word, col, row);
            if (solve(index+1)) {
              return true;
            }
          }
        }        
      }

      return false;
    }

    const retrySolve = (): boolean => {
      wordsArr.unshift(wordsArr.pop() as string);
      crosswordSolved = [...crossword].map(row => row.split(''));
      return solve(0);      
    }

    let result = solve(0);

    let tries = wordsArr.length;
    while (!result && tries > 1) {
      result = retrySolve();
      tries--;
    }

    return crosswordSolved.map(row => row.join(''));

}