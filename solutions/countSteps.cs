static int[] getObstaclesZ(Dictionary<int, HashSet<int>> diction, int baseCol, int baseRec){        
    List<int> arr = new List<int>();

    foreach(KeyValuePair<int, HashSet<int>> pair in diction){
        int delta  = (int)Math.Abs(Convert.ToDecimal(baseRec - pair.Key));
        
        if(pair.Value.Contains(baseCol - delta)){                
            arr.AddRange(new int[]{pair.Key, (baseCol - delta)});               
            break;
        }
        else if(pair.Value.Contains(baseCol + delta)){
            arr.AddRange(new int[]{pair.Key, (baseCol + delta)});               
            break;
        }
    }

    return arr.ToArray();    
}

static int findObstOnX(Dictionary<int, HashSet<int>> obstDict, int col, int rec, int num) {
    int obstX = 0;
    HashSet<int> obstaclesOnX = new HashSet<int>();      
    
    if(obstDict.ContainsKey(rec)){
        obstaclesOnX = obstDict[rec];
        obstDict.Remove(rec);
    }       

    obstaclesOnX = obstaclesOnX.OrderBy(value => value).ToHashSet();
    int foundXR = obstaclesOnX.FirstOrDefault(item => item > col);
    obstaclesOnX.RemoveWhere(item => item > col);
    obstaclesOnX = obstaclesOnX.Reverse().ToHashSet();
    int foundXL = obstaclesOnX.FirstOrDefault();        
    obstX = foundXL;
    if(foundXR > 0) obstX += num - foundXR + 1;        
    
    return obstX;
}

static int findObstOnY(Dictionary<int, HashSet<int>> obstDict, int col, int rec, int num) {
    int obstY = 0;
    HashSet<int> obstaclesOnY = new HashSet<int>();
    List<int> keysRemove = new List<int>(); 
    
    foreach(int key in obstDict.Keys){            
        int c = obstDict[key].FirstOrDefault(val => val == col);
        if(c > 0) {
            obstaclesOnY.Add(key);
            obstDict[key].Remove(c);
            if(obstDict[key].Count == 0) keysRemove.Add(key);
        }            
    }

    foreach(int r in keysRemove){
        obstDict.Remove(r);            
    }        
    keysRemove.Clear();

    obstaclesOnY = obstaclesOnY.OrderBy(value => value).ToHashSet();
    int foundYR = obstaclesOnY.FirstOrDefault(item => item > rec);
    obstaclesOnY.RemoveWhere(item => item > rec);
    obstaclesOnY = obstaclesOnY.Reverse().ToHashSet();
    int foundYL = obstaclesOnY.FirstOrDefault();              
    obstY = foundYL;
    if(foundYR > 0) obstY += num - foundYR + 1;

    return obstY;
}

static int calcObstOnZ(int[] dnLt, int[] dnRt, int[] upLt, int[] upRt, int num){  
    int obstZ = 0;

    if(dnRt.Length > 0)
        obstZ += Math.Min(dnRt[0], num-dnRt[1]+1);
    if(obstZ < 0) obstZ = 0;

    if(dnLt.Length > 0)           
        obstZ += Math.Min(dnLt[0], dnLt[1]);
    if(obstZ < 0) obstZ = 0;

    if(upLt.Length > 0)
        obstZ += Math.Min(num-upLt[0]+1, upLt[1]);
    if(obstZ < 0) obstZ = 0;    

    if(upRt.Length > 0)
        obstZ += Math.Min(num-upRt[0], num-upRt[1])+1;
    if(obstZ < 0) obstZ = 0;

    return obstZ;
}

static int calcAttackOnZ(int rec, int col, int num){
    int cellsZ = Math.Min(rec, rec) - 1;
    if(cellsZ < 0) cellsZ = 0;

    cellsZ += Math.Min(rec-1, num-rec);
    if(cellsZ < 0) cellsZ = 0;

    cellsZ += Math.Min(num-rec, rec-1);
    if(cellsZ < 0) cellsZ = 0;

    cellsZ += Math.Min(num-rec, num-rec);
    if(cellsZ < 0) cellsZ = 0;

    return cellsZ;
}

// Complete the queensAttack function below.
static int queensAttack(int n, int k, int r_q, int c_q, int[][] obstacles) {        
    int attack = n*2 - 2;
    int cellsZ = 0;       
    
    try{

        //quine's attacks Z axis
        cellsZ = calcAttackOnZ(r_q, c_q, n);
        attack += cellsZ;   

        //Obstacles x & y & z axis        
        obstacles = obstacles.OrderBy(item => item[0]).ToArray();     
    
        Dictionary<int, HashSet<int>> obstDict = new Dictionary<int, HashSet<int>>();
        obstDict = obstacles.GroupBy(val => val[0]).ToDictionary(key => key.Key, 
            value => value.Select(val => val[1]).Distinct().ToHashSet());
        Dictionary<int, HashSet<int>> down = new Dictionary<int, HashSet<int>>();
        Dictionary<int, HashSet<int>> up = new Dictionary<int, HashSet<int>>();
        List<int> keysRemove = new List<int>(); 

        //x, y
        int obstX = findObstOnX(obstDict, c_q, r_q, n);
        int obstY = findObstOnY(obstDict, c_q, r_q, n);

        //z
        foreach(int key in obstDict.Keys){
            if(key < r_q){
                down.Add(key, obstDict[key]);
                keysRemove.Add(key);
            }
        }

        foreach(int r in keysRemove){
            obstDict.Remove(r);
        }
        keysRemove.Clear();
        up = obstDict;        
    
        //up right & left
        Dictionary<int, HashSet<int>> uRt = new Dictionary<int, HashSet<int>>();
        Dictionary<int, HashSet<int>> uLt = new Dictionary<int, HashSet<int>>();

        foreach(KeyValuePair<int, HashSet<int>> item in up){
            if(item.Value.FirstOrDefault(c => c > c_q) > 0){                
                uRt.Add(item.Key, item.Value.Where(c => c > c_q).OrderBy(v => v).ToHashSet());
                item.Value.RemoveWhere(c => c > c_q);
                if(item.Value.Count == 0) keysRemove.Add(item.Key);
            }            
        }

        foreach(int r in keysRemove){
            up.Remove(r);
        }
        keysRemove.Clear();

        foreach(KeyValuePair<int, HashSet<int>> item in up){
            if(item.Value.FirstOrDefault(c => c < c_q) > 0){            
                uLt.Add(item.Key, item.Value.OrderBy(v => v).ToHashSet());
            }
        }

        //down right & left       
        down = down.Reverse().ToDictionary(pair => pair.Key, pair => pair.Value);
        Dictionary<int, HashSet<int>> dRt = new Dictionary<int, HashSet<int>>();
        Dictionary<int, HashSet<int>> dLt = new Dictionary<int, HashSet<int>>();
        
        foreach(KeyValuePair<int, HashSet<int>> item in down){          
            if(item.Value.FirstOrDefault(c => c > c_q) > 0){                
                dRt.Add(item.Key, item.Value.Where(c => c > c_q).OrderBy(v => v).ToHashSet());
                item.Value.RemoveWhere(c => c > c_q);                
                if(item.Value.Count == 0) keysRemove.Add(item.Key);
            }            
        }

        foreach(int r in keysRemove){
            down.Remove(r);
        }

        foreach(KeyValuePair<int, HashSet<int>> item in down){
            if(item.Value.FirstOrDefault(c => c < c_q) > 0){        
                dLt.Add(item.Key, item.Value.OrderBy(v => v).ToHashSet());
            }
        }
    
        int[] dnLt = getObstaclesZ(dLt, c_q, r_q);
        int[] dnRt = getObstaclesZ(dRt, c_q, r_q);
        int[] upLt = getObstaclesZ(uLt, c_q, r_q);
        int[] upRt = getObstaclesZ(uRt, c_q, r_q);
    
        int obstZ = calcObstOnZ(dnLt, dnRt, upLt, upRt, n);
    
        //==============
        attack -= obstY;
        attack -= obstX;        
        attack -= obstZ;

        return attack;

    }
    catch(Exception ex){
        return 0;           
    }       

}