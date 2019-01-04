package file;

import jdk.nashorn.internal.runtime.Debug;

import java.io.File;

public class Test {
    public static void main(String[] args){
        File folder = new File("D:\\新建文件夹");
        String keyword = "Repository.cs";
        if (!folder.exists()) { // 如果文件夹不存在
            System.out.println("目录不存在：" + folder.getAbsolutePath());
            return;
        }
        //进行文件过滤，找出所有仓储
        File[] data = FileSearch.searchFile(folder, keyword);// 调用方法获得文件数组
         //文件内容过滤，找出缺orgId的文件
         File[] result = FileFilter.Filter(data);
         for (int i =0; i<result.length;i++){
             File file = result[i];
             System.out.println(file.getAbsolutePath());
         }
    }

}
